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
        var timestamp = DateTime.UtcNow;

        // Ensure the seconds are even
        if (timestamp.Second % 2 == 1)
        {
            timestamp = timestamp.AddSeconds(1);
        }

        var message = new Message
        {
            Timestamp = timestamp,
            Counter = 2
        };

        // Act
        var result = await _processor.ProcessMessage(message);

        // Assert
        Assert.Equal(ProcessingAction.Store, result.Action);
        _mockDatabase.Verify(x => x.StoreMessage(It.IsAny<Message>()), Times.Once);
    }


    // I suspect test is failing cause of ReQueueCounter
    [Fact]
    public async Task ProcessMessage_OddSeconds_ShouldBeRequeued()
    {
        // Arrange
        var timestamp = DateTime.UtcNow;

        // Ensure the seconds are odd
        if (timestamp.Second % 2 == 0)
        {
            timestamp = timestamp.AddSeconds(1);
        }

        var message = new Message
        {
            Timestamp = timestamp,
            Counter = 2,
            ReQueueCounter = 1
        };

        // Act
        var result = await _processor.ProcessMessage(message);

        // Assert
        Assert.Equal(ProcessingAction.Requeue, result.Action);
        Assert.Equal(2, result.Message.Counter); // Counter should be incremented this should møøst definitely have it's own test
    }
} 