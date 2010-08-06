namespace Observal
{
    /// <summary>
    /// An interface implemented by objects that add capabilities to an <see cref="Observer"/>.
    /// </summary>
    public interface IObserverExtension
    {
        /// <summary>
        /// Configures this extension for a given observer. This allows the extension to initialize itself
        /// or add any other extensions that it depends on.
        /// </summary>
        /// <param name="observer">The observer.</param>
        void Configure(Observer observer);

        /// <summary>
        /// Notifies this extension that an item has been added to the current observer.
        /// </summary>
        /// <param name="attachedItem">The attached item that was just added to the observer.</param>
        /// <remarks>
        /// This method is guaranteed to only be called once per item (unless the item is added, removed, 
        /// and added again). 
        /// </remarks>
        void Attach(object attachedItem);

        /// <summary>
        /// Notifies this extension that an item has been removed from the current observer.
        /// </summary>
        /// <param name="detachedItem">The detached item that was just removed from the observer.</param>
        void Detach(object detachedItem);
    }
}