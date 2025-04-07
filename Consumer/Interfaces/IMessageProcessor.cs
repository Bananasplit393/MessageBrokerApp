using Shared.Models;
using Consumer.Processing;

namespace Consumer.Interfaces;
  public interface IMessageProcessor
  {
      Task<ProcessingResult> ProcessMessage(Message message);
  }