﻿var ViewModel = function () {
    var self = this;
    self.books = ko.observableArray();
    self.error = ko.observable();

    var booksUri = '/api/books/';

    function ajaxHelper(uri, method, data) {
        self.error(''); // Clear error message
        return $.ajax({
            type: method,
            url: uri,
            dataType: 'json',
            contentType: 'application/json',
            data: data ? JSON.stringify(data) : null
        }).fail(function (jqXHR, textStatus, errorThrown) {
            self.error(errorThrown);
        });
    }

    function getAllBooks() {
        ajaxHelper(booksUri, 'GET').done(function (data) {
            self.books(data);
        });
    }

    // Fetch the initial data.
    getAllBooks();
};

ko.applyBindings(new ViewModel());

/*In Knockout, the observable class enables data-binding. When the contents of an observable change, the observable notifies all of the data-bound controls, so they can update themselves. (The observableArray class is the array version of observable.) To start with, our view model has two observables:

books holds the list of books.
error contains an error message if an AJAX call fails.
The getAllBooks method makes an AJAX call to get the list of books. Then it pushes the result onto the books array.

The ko.applyBindings method is part of the Knockout library. It takes the view model as a parameter and sets up the data binding.*/