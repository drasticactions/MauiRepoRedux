using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListsCollectionView;

public partial class VListPage : ContentPage
{
    public VListPage()
    {
        InitializeComponent();
        this.BindingContext = new ListViewModel();
    }

    public ListViewModel ViewModel { get; } = new ListViewModel();
}