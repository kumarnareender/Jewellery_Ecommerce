﻿angular.module('Enums', []).
   factory('Enum', [function () {

       var service = {
           monthList: [ { Id: 1, Name: 'January' }, { Id: 2, Name: 'February' }, { Id: 3, Name: 'March' },
                        { Id: 4, Name: 'April' }, { Id: 5, Name: 'May' }, { Id: 6, Name: 'June' },
                        { Id: 7, Name: 'July' }, { Id: 8, Name: 'August' }, { Id: 9, Name: 'September' },
                        { Id: 10, Name: 'October' }, { Id: 11, Name: 'November' }, { Id: 12, Name: 'December' }]
       };
      
       return service;

   }]);