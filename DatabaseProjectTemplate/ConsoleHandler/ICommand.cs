
namespace DatabaseProjectTemplate.ConsoleHandler
{
    public interface ICommand
    {
        public string Command { get; }
        public string Description { get; }

        public abstract bool Execute(ArraySegment<string> args, out string response);
    }
}