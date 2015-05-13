# observal
Automatically exported from code.google.com/p/observal

Observal is a library that helps you to manage complex, hierarchical object models in your WPF applications.

##Getting started

We start by creating an observer, which tracks a set of items:

    var observer = new Observer();

To put the observer to use, we use extensions. For example, the [PropertyChangedExtension](https://github.com/tbayart/observal/blob/wiki/PropertyChangedExtension.md) allows us to be notified any time a property changes on a tracked object:

    observer.Extend(new PropertyChangedExtension())
            .WhenPropertyChanges(x => Console.WriteLine("Property {0} of object {1} changed", x.PropertyName, x.Source));

We can now add items to the observer for tracking:

    observer.Add(customer1);  
    observer.Add(customer2);  
    observer.Add(customer3);

The extension automatically attaches to added items - in this case, it will print whenever an item property is changed.

    customer2.Age = 23;   // Prints 'Property Age of object Customer 2 changed'

##The power of Extensions
Extensions can be combined together to create powerful reactive applications.
The out-of-the-box extensions are:
* [PropertyChangedExtension](https://github.com/tbayart/observal/blob/wiki/PropertyChangedExtension.md), which attaches to the INotifyPropertyChanged.PropertyChanged event
* [TraverseExtension](https://github.com/tbayart/observal/blob/wiki/TraverseExtension.md), which traverses object hierarchies and adds child property values to the observer
* [CollectionExpansionExtension](https://github.com/tbayart/observal/blob/wiki/CollectionExpansionExtension.md), which adds items in ObservableCollections to the observer
* [ItemsChangedExtension](https://github.com/tbayart/observal/blob/wiki/ItemsChangedExtension.md), which invokes a callback when items are added or removed

For example, suppose we have a tree of employees:

    var orgChart = new Employee("Pointy Haired Boss",
      new Employee("Catbert",
      new Employee("Dilbert"),
      new Employee("Wally"),
      new Employee("Alice")));

We can use the [TraverseExtension](https://github.com/tbayart/observal/blob/wiki/TraverseExtension.md) to expand the tree of employees automatically:

    observer.Extend(new TraverseExtension())
            .Follow<Employee>(x => x.DirectReports);

Since DirectReports is a collection, we can use the [CollectionExpansionExtension](https://github.com/tbayart/observal/blob/wiki/CollectionExpansionExtension.md) to add all items in the collection to the observer:

    observer.Extend(new CollectionExpansionExtension());

We can then use [PropertyChangedExtension](https://github.com/tbayart/observal/blob/wiki/PropertyChangedExtension.md) to be notified whenever any employee's property changes:

    observer.Extend(new PropertyChangedExtension())
            .WhenPropertyChanges(x => Console.WriteLine("Employee {0} changed: {1}", x.Source, x.PropertyName));

Now if any employee's property changes, we'll know:

    orgChart.DirectReports[0].DirectReports[1].Title = "Programmer";   // Prints

That goes for newly added objects:

    var newGuy = new Employee();
    orgChart.DirectReports[0].DirectReports[1].Add(newGuy);
    newGuy.IsFired = true;     // Prints
