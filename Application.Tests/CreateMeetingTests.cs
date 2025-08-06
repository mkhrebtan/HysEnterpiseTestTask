using Moq;
using Domain.Entities;
using Domain.Shared;
using Domain.ValueObjects;
using Application.Meetings.Create;

namespace Application.Tests
{
    public class CreateMeetingTests
    {
        private readonly Mock<IRepository> _mockRepository;
        private readonly CreateMeetingCommandHandler _handler;

        public CreateMeetingTests()
        {
            _mockRepository = new Mock<IRepository>();
            _handler = new CreateMeetingCommandHandler(_mockRepository.Object);
        }

        [Fact]
        public async Task Handle_WithPartialOverlaps_FindsEarliestAvailableSlot()
        {
            // Arrange
            var guid1 = Guid.NewGuid();
            var guid2 = Guid.NewGuid();
            var user1 = new User(guid1, "User1");
            var user2 = new User(guid2, "User1");
            
            var existingMeeting1 = new Meeting(new Interval(new DateTime(2024, 1, 1, 10, 0, 0), new DateTime(2024, 1, 1, 11, 30, 0)));
            var existingMeeting2 = new Meeting(new Interval(new DateTime(2024, 1, 1, 14, 0, 0), new DateTime(2024, 1, 1, 15, 0, 0)));

            _mockRepository.Setup(r => r.GetUserById(guid1)).Returns(user1);
            _mockRepository.Setup(r => r.GetUserById(guid2)).Returns(user2);
            _mockRepository.Setup(r => r.GetUserMeetings(user1)).Returns(new List<Meeting> { existingMeeting1 });
            _mockRepository.Setup(r => r.GetUserMeetings(user2)).Returns(new List<Meeting> { existingMeeting2 });

            var command = new CreateMeetingCommand
            {
                ParticipantIds = new List<Guid> { guid1, guid2 },
                EarliestStart = new DateTime(2024, 1, 1, 9, 0, 0),
                LatestEnd = new DateTime(2024, 1, 1, 17, 0, 0),
                DurationInMinutes = 60
            };

            // Act
            var result = await _handler.Handle(command);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(new DateTime(2024, 1, 1, 9, 0, 0), result.Value.Start);
            Assert.Equal(new DateTime(2024, 1, 1, 10, 0, 0), result.Value.End);
        }

        [Fact]
        public async Task Handle_WithBackToBackMeetings_FindsSlotAfterAllMeetings()
        {
            // Arrange
            var guid1 = Guid.NewGuid();
            var user1 = new User(guid1, "User1");
            
            var meeting1 = new Meeting(new Interval(new DateTime(2024, 1, 1, 9, 0, 0), new DateTime(2024, 1, 1, 10, 0, 0)));
            var meeting2 = new Meeting(new Interval(new DateTime(2024, 1, 1, 10, 0, 0), new DateTime(2024, 1, 1, 11, 0, 0)));
            var meeting3 = new Meeting(new Interval(new DateTime(2024, 1, 1, 11, 0, 0), new DateTime(2024, 1, 1, 12, 0, 0)));

            _mockRepository.Setup(r => r.GetUserById(guid1)).Returns(user1);
            _mockRepository.Setup(r => r.GetUserMeetings(user1)).Returns(new List<Meeting> { meeting1, meeting2, meeting3 });

            var command = new CreateMeetingCommand
            {
                ParticipantIds = new[] { guid1 },
                EarliestStart = new DateTime(2024, 1, 1, 9, 0, 0),
                LatestEnd = new DateTime(2024, 1, 1, 15, 0, 0),
                DurationInMinutes = 30
            };

            // Act
            var result = await _handler.Handle(command);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(new DateTime(2024, 1, 1, 12, 0, 0), result.Value.Start);
            Assert.Equal(new DateTime(2024, 1, 1, 12, 30, 0), result.Value.End);
        }

        [Fact]
        public async Task Handle_WithNoAvailableTimeSlot_ReturnsFailure()
        {
            // Arrange
            var guid1 = Guid.NewGuid();
            var user1 = new User(guid1, "User1");

            var meeting1 = new Meeting(new Interval(new DateTime(2024, 1, 1, 9, 0, 0), new DateTime(2024, 1, 1, 12, 0, 0)));
            var meeting2 = new Meeting(new Interval(new DateTime(2024, 1, 1, 13, 0, 0), new DateTime(2024, 1, 1, 17, 0, 0)));

            _mockRepository.Setup(r => r.GetUserById(guid1)).Returns(user1);
            _mockRepository.Setup(r => r.GetUserMeetings(user1)).Returns(new List<Meeting> { meeting1, meeting2 });

            var command = new CreateMeetingCommand
            {
                ParticipantIds = new[] { guid1 },
                EarliestStart = new DateTime(2024, 1, 1, 8, 0, 0),
                LatestEnd = new DateTime(2024, 1, 1, 18, 0, 0),
                DurationInMinutes = 120 // 2 hours - won't fit in 1-hour gap
            };

            // Act
            var result = await _handler.Handle(command);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("CreateMeeting.NoAvailableSlot", result.Error.Code);
        }

        [Fact]
        public async Task Handle_WithOverlappingMeetings_MergesIntervalsCorrectly()
        {
            // Arrange
            var guid1 = Guid.NewGuid();
            var guid2 = Guid.NewGuid();
            var user1 = new User(guid1, "User1");
            var user2 = new User(guid2, "User1");

            var meeting1 = new Meeting (new Interval(new DateTime(2024, 1, 1, 10, 0, 0), new DateTime(2024, 1, 1, 12, 0, 0)));
            var meeting2 = new Meeting (new Interval(new DateTime(2024, 1, 1, 11, 0, 0), new DateTime(2024, 1, 1, 13, 0, 0)));

            _mockRepository.Setup(r => r.GetUserById(guid1)).Returns(user1);
            _mockRepository.Setup(r => r.GetUserById(guid2)).Returns(user2);
            _mockRepository.Setup(r => r.GetUserMeetings(user1)).Returns(new List<Meeting> { meeting1 });
            _mockRepository.Setup(r => r.GetUserMeetings(user2)).Returns(new List<Meeting> { meeting2 });

            var command = new CreateMeetingCommand
            {
                ParticipantIds = new[] { guid1, guid2 },
                EarliestStart = new DateTime(2024, 1, 1, 10, 0, 0),
                LatestEnd = new DateTime(2024, 1, 1, 16, 0, 0),
                DurationInMinutes = 60
            };

            // Act
            var result = await _handler.Handle(command);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(new DateTime(2024, 1, 1, 13, 0, 0), result.Value.Start);
        }

        [Fact]
        public async Task Handle_WithNonExistentParticipant_ReturnsFailure()
        {
            // Arrange
            var guid1 = Guid.NewGuid();
            var guid2 = Guid.NewGuid();
            var user1 = new User(guid1, "User1");

            _mockRepository.Setup(r => r.GetUserById(guid1)).Returns(user1);
            _mockRepository.Setup(r => r.GetUserById(guid2)).Returns((User?)null);

            var command = new CreateMeetingCommand
            {
                ParticipantIds = new[] { guid1, guid2 },
                EarliestStart = new DateTime(2024, 1, 1, 9, 0, 0),
                LatestEnd = new DateTime(2024, 1, 1, 17, 0, 0),
                DurationInMinutes = 60
            };

            // Act
            var result = await _handler.Handle(command);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("CreateMeeting.InvalidParticipants", result.Error.Code);
        }

        [Fact]
        public async Task Handle_WithNoExistingMeetings_ReturnsEarliestStartTime()
        {
            // Arrange
            var guid1 = Guid.NewGuid();
            var user1 = new User(guid1, "User1");

            _mockRepository.Setup(r => r.GetUserById(guid1)).Returns(user1);
            _mockRepository.Setup(r => r.GetUserMeetings(user1)).Returns(new List<Meeting>());

            var command = new CreateMeetingCommand
            {
                ParticipantIds = new[] { guid1 },
                EarliestStart = new DateTime(2024, 1, 1, 9, 0, 0),
                LatestEnd = new DateTime(2024, 1, 1, 17, 0, 0),
                DurationInMinutes = 60
            };

            // Act
            var result = await _handler.Handle(command);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(new DateTime(2024, 1, 1, 9, 0, 0), result.Value.Start);
            Assert.Equal(new DateTime(2024, 1, 1, 10, 0, 0), result.Value.End);
        }

        [Fact]
        public async Task Handle_WithMeetingExactlyAtBoundary_FindsSlotBeforeOrAfter()
        {
            // Arrange
            var guid1 = Guid.NewGuid();
            var user1 = new User(guid1, "User1");

            var meeting1 = new Meeting(new Interval(new DateTime(2024, 1, 1, 12, 0, 0), new DateTime(2024, 1, 1, 13, 0, 0)));

            _mockRepository.Setup(r => r.GetUserById(guid1)).Returns(user1);
            _mockRepository.Setup(r => r.GetUserMeetings(user1)).Returns(new List<Meeting> { meeting1 });

            var command = new CreateMeetingCommand
            {
                ParticipantIds = new[] { guid1 },
                EarliestStart = new DateTime(2024, 1, 1, 11, 0, 0),
                LatestEnd = new DateTime(2024, 1, 1, 14, 0, 0),
                DurationInMinutes = 60
            };

            // Act
            var result = await _handler.Handle(command);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(new DateTime(2024, 1, 1, 11, 0, 0), result.Value.Start);
        }

        [Fact]
        public async Task Handle_WithTightTimeConstraint_FindsExactFit()
        {
            // Arrange
            var guid1 = Guid.NewGuid();
            var user1 = new User(guid1, "User1");

            var meeting1 = new Meeting(new Interval(new DateTime(2024, 1, 1, 10, 0, 0), new DateTime(2024, 1, 1, 11, 0, 0)));
            var meeting2 = new Meeting(new Interval(new DateTime(2024, 1, 1, 12, 0, 0), new DateTime(2024, 1, 1, 13, 0, 0)));

            _mockRepository.Setup(r => r.GetUserById(guid1)).Returns(user1);
            _mockRepository.Setup(r => r.GetUserMeetings(user1)).Returns(new List<Meeting> { meeting1, meeting2 });

            var command = new CreateMeetingCommand
            {
                ParticipantIds = new[] { guid1 },
                EarliestStart = new DateTime(2024, 1, 1, 10, 0, 0),
                LatestEnd = new DateTime(2024, 1, 1, 14, 0, 0),
                DurationInMinutes = 60
            };

            // Act
            var result = await _handler.Handle(command);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(new DateTime(2024, 1, 1, 11, 0, 0), result.Value.Start);
            Assert.Equal(new DateTime(2024, 1, 1, 12, 0, 0), result.Value.End);
        }
    }
}
