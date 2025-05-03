using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using WorkoutApp.Models;
using WorkoutApp.Service;

public class CategoryFilterViewModel : INotifyPropertyChanged
{
    private readonly CategoryService categoryService;
    private Category selectedCategory;

    public ObservableCollection<Category> Categories { get; } = new();

    public Category SelectedCategory
    {
        get => selectedCategory;
        set
        {
            if (selectedCategory != value)
            {
                selectedCategory = value;
                OnPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public CategoryFilterViewModel(CategoryService categoryService)
    {
        this.categoryService = categoryService;
    }

    public async Task LoadCategoriesAsync()
    {
        var categories = await categoryService.GetAllAsync();

        Categories.Clear();
        foreach (var category in categories)
        {
            Categories.Add(category);
        }
    }

    protected void OnPropertyChanged([CallerMemberName] string name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
