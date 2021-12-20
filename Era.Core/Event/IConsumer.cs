using System.Threading;
using System.Threading.Tasks;

namespace Era.Core.Event
{
    public interface IConsumer<T>
    {
        Task Handle(T @event, CancellationToken cancellationToken);
    }
}
