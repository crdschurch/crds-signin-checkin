import { SetupComponent } from './setup.component';
import { Observable } from 'rxjs/Observable';

let fixture: SetupComponent;

let routerStub: any = {
  navigate(): void {
  }
};

let setupServiceStup: any = {
  getChildrenByPhoneNumber() {
    return Observable.of([1, 2, 3]);
  }
};

let rootServiceStub: any = {
  announceEvent(): void {
  }
};

describe('SetupComponent', () => {

  beforeEach(() => {
    fixture = new SetupComponent(setupServiceStup, routerStub, rootServiceStub);
  });

  describe('#ngOnInit', () => {
  });

  describe('#reset', () => {
    beforeEach(() => {
      fixture.machineId = 'test-id';
    });

    it('should reset machineId to undefined', () => {
      expect(fixture.machineId).toEqual('test-id');
      fixture.reset();
      expect(fixture.machineId).toEqual(undefined);
    });
  });
});

