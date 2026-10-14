using System.CommandLine;
using System.CommandLine.Parsing;
using VeeamVhcAws.Commands;
using Xunit;

namespace VeeamVhcAws.Tests.Commands;

public class ServeBindTests
{
    [Fact]
    public void Serve_ListensOnLoopbackUnlessBindIsSet()
    {
        var command = ServeCommand.Create();
        var bind = Assert.IsAssignableFrom<Option<string>>(
            command.Options.Single(o => o.Aliases.Contains("--bind")));

        var omitted = command.Parse("--port 9100");
        Assert.Equal("127.0.0.1", omitted.GetValueForOption(bind));

        var chosen = command.Parse("--bind 10.1.2.3 --port 9100");
        Assert.Equal("10.1.2.3", chosen.GetValueForOption(bind));

        var help = new StringWriter();
        var previous = Console.Out;
        try
        {
            Console.SetOut(help);
            Assert.Equal(0, command.Invoke("--help"));
        }
        finally
        {
            Console.SetOut(previous);
        }

        var text = help.ToString();
        Assert.Contains("--bind", text);
        Assert.Contains("127.0.0.1", text);
        Assert.Contains("non-loopback", text);
    }
}
