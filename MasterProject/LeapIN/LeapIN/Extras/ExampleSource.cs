using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeapIN.Extras
{
    class ExampleSource
    {
        /* The following is an example of the Command and Property Change classes
         * 
         * The command class takes two parameters, the first is what function to execute,
         * the second is a condition on where it can execute. They both follow the same pattern
         * of passing a param like below just put a comma in between.
         * 
         * This functions as a property of a class so that the data that holds which function
         * is called is hidden in the class (Encapsulation). Most data in WPF is accessed through 'Properties'
         */

        //private ICommand _importCommand;

        //public ICommand ImportCommand
        //{
        //    get
        //    {
        //        if (_importCommand == null)
        //        {
        //            _importCommand = new RelayCommand(
        //                param => ImportNames());
        //        }
        //        return _importCommand;
        //    }
        //}


        /* To implement PropertyChanged events you need to derive it in your UI classes
         * 
         * The PropertyChange class is a single event handler, 
         * its job is to take a particular property name and update the UI element this property is linked to.
         * 
         * You might have a list of items and when you select an item in the list and you want to show a page related to that item
         * you need to call OnPropertyChanged, this will update any UI things linked to 'SelectedItem'. Notice it is in the set part of the property.
         * 
         * This might not be needed for our project however it's a handy thing to have around and know about for WPF.
         */

        //public class ImportClass : PropertyChange

        //private Item _selectedItem = null;

        //public Item SelectedItem
        //{
        //    get { return _selectedItem; }
        //    set
        //    {
        //        if (_selectedItem != value)
        //        {
        //            _selectedItem = value;
        //            OnPropertyChanged("SelectedItem");
        //        }
        //    }
        //}

    }
}
