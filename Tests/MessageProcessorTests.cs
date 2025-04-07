using Xunit;
using Moq;
using Consumer;
using Consumer.Processing;
using Shared.Models;

namespace Tests;

public class MessageProcessorTests
{
    private readonly Mock<IDatabaseService> _mockDatabase;
    private readonly MessageProcessor _processor;

    public MessageProcessorTests()
    {
        _mockDatabase = new Mock<IDatabaseService>();
        _processor = new MessageProcessor(_mockDatabase.Object);
    }

    [Fact]
    public async Task ProcessMessage_OldMessage_ShouldBeDiscarded()
    {
        // Arrange
        var oldMessage = new Message
        {
            Timestamp = DateTime.UtcNow.AddMinutes(-2),
            Counter = 1
        };

        // Act
        var result = await _processor.ProcessMessage(oldMessage);

        // Assert
        Assert.Equal(ProcessingAction.Discard, result.Action);
    }

    [Fact]
    public async Task ProcessMessage_EvenSeconds_ShouldBeStored()
    {
        // Arrange
        var message = new Message
        {
            // Even second
            Timestamp = new DateTime(2025, 4, 7, 12, 0, 2), // April 7, 2025, 12:00:02 PM
            Counter = 1
        };

        // Act
        var result = await _processor.ProcessMessage(message);

        // Assert
        Assert.Equal(ProcessingAction.Store, result.Action);
        _mockDatabase.Verify(x => x.StoreMessage(It.IsAny<Message>()), Times.Once);
    }

    [Fact]
    public async Task ProcessMessage_OddSeconds_ShouldBeRequeued()
    {
        // Arrange
        var message = new Message
        {
            // Odd second
            Timestamp = new DateTime(2025, 4, 7, 12, 0, 1), // April 7, 2025, 12:00:01 PM
            Counter = 1
        };

        // Act
        var result = await _processor.ProcessMessage(message);

        // Assert
        Assert.Equal(ProcessingAction.Requeue, result.Action);
        Assert.Equal(2, result.Message.Counter); // Counter should be incremented
    }
} 