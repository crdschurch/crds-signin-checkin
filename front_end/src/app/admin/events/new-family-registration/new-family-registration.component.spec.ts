import { Observable } from 'rxjs';
import { NewFamilyRegistrationComponent } from './new-family-registration.component';
import { Router, ActivatedRoute, ActivatedRouteSnapshot } from '@angular/router';
import { NewFamily, NewParent, NewChild } from '../../../../shared/models';
import * as moment from 'moment';

const eventId = '92398420398';
let router;
let apiService = jasmine.createSpyObj('apiService', ['getEvent']);
let headerService;
let route: ActivatedRoute;
route = new ActivatedRoute();
route.snapshot = new ActivatedRouteSnapshot();
route.snapshot.params = { eventId: eventId };

fdescribe('NewFamilyRegistrationComponent', () => {
  it('#ngOnInit', () => {
    (<jasmine.Spy>(apiService.getEvent)).and.returnValue(Observable.of([{}]));
    let fixture = new NewFamilyRegistrationComponent(route, apiService, headerService, router);
    fixture.ngOnInit();
    expect(apiService.getEvent).toHaveBeenCalledWith(eventId);
  });

});
