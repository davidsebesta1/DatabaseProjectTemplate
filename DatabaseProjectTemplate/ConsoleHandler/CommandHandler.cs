using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseProjectTemplate.ConsoleHandler
{
    public static class CommandHandler
    {
        private static Dictionary<string, ICommand> _registeredCommands;

        public static bool TryExecuteCommand(string commandName, ArraySegment<string> args, out string response)
        {
            if (_registeredCommands == null)
            {
                response = "Command not found";
                return false;
            }

            if (_registeredCommands.TryGetValue(commandName, out ICommand command))
            {
                return command.Execute(args, out response);
            }

            response = "Command not found";
            return false;
        }

        private static bool TryRegisterCommand(Type type)
        {
            if (Activator.CreateInstance(type) is not ICommand command)
            {
                return false;
            }

            _registeredCommands.Add(command.Command, command);
            return true;
        }

        public static bool TryRegisterCommand<T>() where T : ICommand
        {
            return TryRegisterCommand(typeof(T));
        }
    }
}
