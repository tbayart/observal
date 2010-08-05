namespace Observal
{
    public interface IObserverExtension
    {
        void Configure(Observer observer);
        void Attach(Observer observer, object attachedItem);
        void Detach(Observer observer, object detachedItem);
    }
}