// Statics Our service handler, handleError, logs the response to the console, transforms the error into a user-friendly message,
// and returns the message in a new, failed observable via Observable.throw.
import 'rxjs/add/observable/throw';

// Chained to a map. We use the Observable catch operator on the service level. 
// It takes an error handling function with an error object as the argument.
import 'rxjs/add/operator/catch';

// Emits an item from the source Observable after a particular timespan has passed without the Observable omitting any other items.
import 'rxjs/add/operator/debounceTime';

// Returns an observable sequence that contains only distinct contiguous elements according to the keySelector and the comparer.
import 'rxjs/add/operator/distinctUntilChanged';

// Very similar to a "then" on a Promise except this is an Observable
import 'rxjs/add/operator/map';

// Used to return only the most recent request
import 'rxjs/add/operator/switchMap';
