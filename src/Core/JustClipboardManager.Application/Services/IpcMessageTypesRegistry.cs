using System.Reflection;
using Google.Protobuf;
using Google.Protobuf.Reflection;
using JustClipboardManager.Application.IPC.Commands;
using JustClipboardManager.Application.IPC.Common;
using JustClipboardManager.Application.IPC.Events;

namespace JustClipboardManager.Application.Services;

internal interface IIpcMessageTypesRegistry
{
  TypeRegistry CommandParametersRegistry { get; }
  TypeRegistry CommandResultsRegistry { get; }
  TypeRegistry EventPayloadsRegistry { get; }
}


internal class IpcMessageTypesRegistry : IIpcMessageTypesRegistry
{
  public IpcMessageTypesRegistry()
  {
    var ipcTypes = Assembly.GetExecutingAssembly()
      .GetTypes()
      .Where(
        t => t is { IsClass: true, IsAbstract: false, IsSealed: true }
             && t.GetInterfaces().Any(
               i => i.IsGenericType
                    && i.GetGenericTypeDefinition() == typeof(IMessage<>)
                    && i.GetGenericArguments().FirstOrDefault() == t
             )
      );
    var commandParameterDescriptors = new List<MessageDescriptor>([AppCommandRequest.Descriptor]);
    var commandResultDescriptors = new List<MessageDescriptor>([AppCommandResult.Descriptor]);
    var eventPayloadDescriptors = new List<MessageDescriptor>([AppEvent.Descriptor]);
    foreach (var type in ipcTypes)
    {
      var propertyInfo = type.GetProperty(nameof(IMessage.Descriptor), BindingFlags.Public | BindingFlags.Static);
      var descriptor = propertyInfo?.GetValue(null) as MessageDescriptor;
      if (descriptor is null)
      {
        continue;
      }

      if (type.Name == nameof(Empty))
      {
        commandParameterDescriptors.Add(descriptor);
        commandResultDescriptors.Add(descriptor);
      }
      else if (type.Name.EndsWith("Parameter"))
      {
        commandParameterDescriptors.Add(descriptor);
      }
      else if (type.Name.EndsWith("Result"))
      {
        commandResultDescriptors.Add(descriptor);
      }
      else if (type.Name.EndsWith("Payload"))
      {
        eventPayloadDescriptors.Add(descriptor);
      }
    }

    CommandParametersRegistry = TypeRegistry.FromMessages(commandParameterDescriptors);
    CommandResultsRegistry = TypeRegistry.FromMessages(commandResultDescriptors);
    EventPayloadsRegistry = TypeRegistry.FromMessages(eventPayloadDescriptors);
  }

  public TypeRegistry CommandParametersRegistry { get; }
  public TypeRegistry CommandResultsRegistry { get; }
  public TypeRegistry EventPayloadsRegistry { get; }
}
