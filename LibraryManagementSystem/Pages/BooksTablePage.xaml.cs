using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using LibraryManagementSystem.Data.Models;
using LibraryManagementSystem.Services;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LibraryManagementSystem.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class BooksTablePage : Page
{
    private LibraryService _libraryService;
    public ObservableCollection<Book> Books { get; private set; } = [];
    private bool _inEditMode;
    private Book? _book;
    public BooksTablePage()
    {
        InitializeComponent();
        DataContext = this;

        NavigationCacheMode = NavigationCacheMode.Required;
    }
    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        await LoadBooksAsync();
    }
    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        if (Frame.CanGoBack)
            Frame.GoBack();
    }
    private async Task LoadBooksAsync()
    {
        LoadingRing.IsActive = true;

        if (_libraryService == null)
        {
            await Task.Run(() =>
            {
                _libraryService = new LibraryService();
            });
        }

        var books = await _libraryService!.GetBooksAsync();

        Books.Clear();
        foreach (var book in books)
            Books.Add(book);

        LoadingRing.IsActive = false;
    }
    private async void EditButton_ClickAsync(object sender, RoutedEventArgs e)
    {
        _inEditMode = true;
        _book = (sender as Button)?.Tag as Book;
        if (_book == null)
            return;

        BookDialog.Title = "Edit Book Details";
        BookDialog.PrimaryButtonText = "Save";

        TitleBox.Text = _book.Title;
        AuthorBox.Text = _book.Author;
        GenreBox.Text = _book.Genre;
        YearPublishedBox.Text = _book.YearPublished.ToString();
        QuantityBox.Text = _book.Quantity.ToString();

        await BookDialog.ShowAsync();
    }
    private async void AddButton_ClickAsync(object sender, RoutedEventArgs e)
    {
        _inEditMode = false;
        _book = null;

        BookDialog.Title = "Add New Book";
        BookDialog.PrimaryButtonText = "Add";

        TitleBox.Text = "";
        AuthorBox.Text = "";
        GenreBox.Text = "";
        YearPublishedBox.Text = "";
        QuantityBox.Text = "";

        await BookDialog.ShowAsync();
    }
    private async void BookDialogPrimaryButton_Clicked(ContentDialog sender, ContentDialogButtonClickEventArgs e)
    { 
        if (_inEditMode)
        {
            _libraryService!.UpdateBook(_book!.BookId, TitleBox.Text, AuthorBox.Text, GenreBox.Text, int.Parse(YearPublishedBox.Text), int.Parse(QuantityBox.Text));
        }
        else
        {
            _libraryService!.AddBook(TitleBox.Text, AuthorBox.Text, GenreBox.Text, int.Parse(YearPublishedBox.Text), int.Parse(QuantityBox.Text));
        }

        await LoadBooksAsync();
    }
    private async void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        _book = (sender as Button)?.Tag as Book;
        if (_book == null)
            return;

        DeleteDialogText.Text = $"Are you sure you want to delete \"{_book.Title}\"?";

        DeleteBookConfirmDialog.XamlRoot = this.XamlRoot;

        await DeleteBookConfirmDialog.ShowAsync();
    }
    private async void DeleteBookDialogPrimaryButton_Clicked(ContentDialog sender, ContentDialogButtonClickEventArgs e)
    { 
        _libraryService.DeleteBook(_book!.BookId);
        await LoadBooksAsync();
    }
}