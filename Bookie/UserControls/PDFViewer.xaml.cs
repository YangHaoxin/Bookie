﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Bookie.UserControls
{
    using System.ComponentModel;

    using Bookie.Common.Model;
    using Bookie.Core.Domains;

    using System.Linq;

    using Bookie.Common;
    using Bookie.Views;

    /// <summary>
    /// Interaction logic for PDFViewer.xaml
    /// </summary>
    public partial class PDFViewer : UserControl, INotifyPropertyChanged
    {

        public Book PDFUrl
        {
            get
            {
                return (Book)GetValue(PDFUrlProperty);
            }
            set
            {
                SetValueDp(PDFUrlProperty, value);
                NotifyPropertyChanged("PDFUrl");
            }
        }

        private Visibility _leftPane;

        private Visibility _rightPane;

        public Visibility LeftPane
        {
            get
            {
                return _leftPane;
            }
            set
            {
                _leftPane = value;
                NotifyPropertyChanged("LeftPane");
            }
        }


        public Visibility RightPane
        {
            get
            {
                return _rightPane;
            }
            set
            {
                _rightPane = value;
                NotifyPropertyChanged("RightPane");
            }
        }

        private BookMark _selectedBookMark;

        public BookMark SelectedBookMark
        {
            get
            {
                return _selectedBookMark;
            }
            set
            {
                _selectedBookMark = value;
                NotifyPropertyChanged("SelectedBookMark");
                if (_selectedBookMark != null)
                {
                    moonPdfPanel.GotoPage(_selectedBookMark.BookMarkedPage);
                }
            }
        }

        private Note _selectedNote;

        public Note SelectedNote
        {
            get
            {
                return _selectedNote;
            }
            set
            {
                _selectedNote = value;
                NotifyPropertyChanged("SelectedNote");
                if (_selectedNote != null && _selectedNote.PageNumber != null)
                {
                    moonPdfPanel.GotoPage(Convert.ToInt32(_selectedNote.PageNumber));
                }
            }
        }

        private ICommand _deleteBookmarkCommand;
        public ICommand DeleteBookMarkCommand
        {
            get
            {
                return _deleteBookmarkCommand
                       ?? (_deleteBookmarkCommand = new RelayCommand((parameter) => DeleteBookMark(parameter), p => true));

            }
        }



        private ICommand _editNoteCommand;
        public ICommand EditNoteCommand
        {
            get
            {
                return _editNoteCommand
                       ?? (_editNoteCommand = new RelayCommand((parameter) => EditNote(parameter), p => true));

            }
        }



        private ICommand _rightPaneCommand;
        private ICommand _leftPaneCommand;
        public ICommand LeftPaneCommand
        {
            get
            {
                return _leftPaneCommand
                       ?? (_leftPaneCommand = new RelayCommand(p => LeftPaneToggle(), p => true));
            }
        }

        public ICommand RightPaneCommand
        {
            get
            {
                return _rightPaneCommand
                       ?? (_rightPaneCommand = new RelayCommand(p => RightPaneToggle(), p => true));
            }
        }

        private int currentPage;


        public static readonly DependencyProperty PDFUrlProperty = DependencyProperty.Register(
            "PDFUrl",
            typeof(Book),
            typeof(PDFViewer),
            null);

        public event PropertyChangedEventHandler PropertyChanged;


        private void LeftPaneToggle()
        {
            LeftPane = LeftPane == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;

        }

        private void RightPaneToggle()
        {
            RightPane = RightPane == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }


        private void SetValueDp(
            DependencyProperty property,
            object value,
            [System.Runtime.CompilerServices.CallerMemberName] String p = null)
        {
            SetValue(property, value);
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(p));
            }
        }

        public PDFViewer()
        {
            InitializeComponent();
            SelectedBookMark = null;
            SelectedNote = null;
            NoteLabel.Visibility = Visibility.Hidden;
            BookMarkLabel.Visibility = Visibility.Hidden;
            (Content as FrameworkElement).DataContext = this;
            moonPdfPanel.PageChangedEvent += moonPdfPanel_PageChangedEvent;

        }

        void moonPdfPanel_PageChangedEvent(object sender, EventArgs e)
        {
            currentPage = moonPdfPanel.GetCurrentPageNumber();
            pageCount.Content = moonPdfPanel.GetCurrentPageNumber() + "/" + moonPdfPanel.TotalPages;
            var b = PDFUrl.Notes.FirstOrDefault(x => x.PageNumber == currentPage);
            if (b != null)
            {
                SelectedNote = b;
                NoteLabel.Visibility = Visibility.Visible;
            }
            else
            {
                SelectedNote = null;
                NoteLabel.Visibility = Visibility.Hidden;
            }

            var c = PDFUrl.BookMarks.FirstOrDefault(x => x.BookMarkedPage == currentPage);
            if (c != null)
            {
                SelectedBookMark = c;
                BookMarkLabel.Visibility = Visibility.Visible;
            }
            else
            {
                SelectedBookMark = null;
                BookMarkLabel.Visibility = Visibility.Hidden;
            }



        }

        public void Refresh()
        {
            PDFUrl = new BookDomain().GetBookById(PDFUrl.Id);
        }

        public void OpenPDF(string url)
        {
            moonPdfPanel.OpenFile(url);
            pageCount.Content = moonPdfPanel.GetCurrentPageNumber() + "/" + moonPdfPanel.TotalPages;
        }

        private void ButtonFirstPage(object sender, RoutedEventArgs e)
        {
            moonPdfPanel.GotoFirstPage();
        }

        private void ButtonPreviousPage(object sender, RoutedEventArgs e)
        {
            moonPdfPanel.GotoPreviousPage();
        }

        private void ButtonNextPage(object sender, RoutedEventArgs e)
        {
            moonPdfPanel.GotoNextPage();
        }

        private void ButtonLastPage(object sender, RoutedEventArgs e)
        {
            moonPdfPanel.GotoLastPage();
        }

        private void ButtonZoomWidth(object sender, RoutedEventArgs e)
        {
            moonPdfPanel.ZoomToWidth();
        }

        private void ButtonZoomHeight(object sender, RoutedEventArgs e)
        {
            moonPdfPanel.ZoomToHeight();
        }

        private void ButtonSinglePage(object sender, RoutedEventArgs e)
        {
            moonPdfPanel.PageRowDisplay = MoonPdfLib.PageRowDisplayType.SinglePageRow;
        }

        private void ButtonContinuous(object sender, RoutedEventArgs e)
        {
            moonPdfPanel.PageRowDisplay = MoonPdfLib.PageRowDisplayType.ContinuousPageRows;
        }

        private void GotoPage_OnClick(object sender, RoutedEventArgs e)
        {
            if (gotopage.Text != "")
            {
                moonPdfPanel.GotoPage(Int32.Parse(gotopage.Text));
            }
        }

        private void ButtonZoomIn(object sender, RoutedEventArgs e)
        {
            moonPdfPanel.ZoomIn();
        }

        private void ButtonZoomOut(object sender, RoutedEventArgs e)
        {
            moonPdfPanel.ZoomOut();
        }

        private void moonPdfPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void ButtonAddBookMark(object sender, RoutedEventArgs e)
        {
            foreach (var x in PDFUrl.BookMarks)
            {
                x.EntityState = EntityState.Unchanged;
            }
            foreach (var publisher in PDFUrl.Publishers)
            {
                publisher.EntityState = EntityState.Unchanged;
            }
            foreach (var author in PDFUrl.Authors)
            {
                author.EntityState = EntityState.Unchanged;
            }



            BookMark b = new BookMark
                             {
                                 Book = PDFUrl,
                                 BookMarkedPage = moonPdfPanel.GetCurrentPageNumber(),
                                 EntityState = EntityState.Added
                 };
            PDFUrl.BookMarks.Add(b);
            new BookDomain().UpdateBook(PDFUrl);
            Refresh();
        }


        public void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        private void ButtonAddNote(object sender, RoutedEventArgs e)
        {
            //Add note
            NoteView nv = new NoteView(PDFUrl, moonPdfPanel.GetCurrentPageNumber(), null);
            nv._viewModel.NoteChanged += _viewModel_NoteChanged;

            nv.ShowDialog();
        }

        private void EditNote(object parameter)
        {
            NoteView nv = new NoteView(PDFUrl, moonPdfPanel.GetCurrentPageNumber(), (Note)parameter);
            nv._viewModel.NoteChanged += _viewModel_NoteChanged;
            nv.ShowDialog();
        }



        private void DeleteBookMark(object parameter)
        {
            foreach (var x in PDFUrl.BookMarks)
            {
                x.EntityState = EntityState.Unchanged;
            }
            foreach (var publisher in PDFUrl.Publishers)
            {
                publisher.EntityState = EntityState.Unchanged;
            }
            foreach (var author in PDFUrl.Authors)
            {
                author.EntityState = EntityState.Unchanged;
            }

            var bb = (BookMark)parameter;
            bb.EntityState = EntityState.Deleted;


            PDFUrl.BookMarks.ToList().RemoveAll(x => x.Id == bb.Id);
            new BookDomain().UpdateBook(PDFUrl);
            Refresh();
        }



        void _viewModel_NoteChanged(object sender, EventArgs e)
        {
            Refresh();
        }

        private void BookMarkLabel_OnClick(object sender, RoutedEventArgs e)
        {
            if (LeftPane == Visibility.Collapsed)
            {
                LeftPane = Visibility.Visible;
            }
        }

        private void NoteLabel_OnClick(object sender, RoutedEventArgs e)
        {
            if (RightPane == Visibility.Collapsed)
            {
                RightPane = Visibility.Visible;
            }
            EditNote(SelectedNote);
        }

        private void ButtonBase9_OnClick(object sender, RoutedEventArgs e)
        {
            Refresh();
        }


    }
}