import { SetupComponent } from './setup.component';
import { Observable } from 'rxjs/Observable';

let fixture: SetupComponent;

let setupServiceStub: any = {
  setMachineIdConfigCookie: {}
  getMachineIdConfigCookie: {}
};

let rootServiceStub: any = {
  announceEvent(): void {
  }
};

describe('SetupComponent', () => {
  describe('#ngOnInit', () => {
    let setMachineIdConfigCookie;
    let getMachineIdConfigCookie;

    beforeEach(() => {
      setMachineIdConfigCookie = spyOn(setupServiceStub, 'setMachineIdConfigCookie');
      getMachineIdConfigCookie = spyOn(setupServiceStub, 'getMachineIdConfigCookie').and.returnValue(undefined);
    });

    it('should get the param values and have no machine id', () => {

      let routerStub: any = {
        params: Observable.of({'error': false, 'machine': undefined}),
        navigate(): void {
        }
      };

      fixture = new SetupComponent(setupServiceStub, routerStub, rootServiceStub);
      fixture.ngOnInit();

      expect(fixture.isError).toBeFalsy();
      expect(fixture.machineId).toEqual(undefined);
      expect(setMachineIdConfigCookie).not.toHaveBeenCalled();
      expect(getMachineIdConfigCookie).toHaveBeenCalled();
    });

    it('should get the param values and set machine id', () => {

      let routerStub: any = {
        params: Observable.of({'error': true, 'machine': 'xyz'}),
        navigate(): void {
        }
      };

      fixture = new SetupComponent(setupServiceStub, routerStub, rootServiceStub);
      fixture.ngOnInit();

      expect(fixture.isError).toBeTruthy();
      expect(fixture.machineId).toEqual('xyz');
      expect(setMachineIdConfigCookie).toHaveBeenCalled();
      expect(getMachineIdConfigCookie).not.toHaveBeenCalled();
    });
  });

  describe('#reset', () => {
    beforeEach(() => {
      let routerStub: any = {
        params: Observable.of({'error': true, 'machine': 'xyz'}),
        navigate(): void {
        }
      };

      fixture = new SetupComponent(setupServiceStub, routerStub, rootServiceStub);
      fixture.machineId = 'test-id';
    });

    it('should reset machineId to undefined', () => {
      expect(fixture.machineId).toEqual('test-id');
      fixture.reset();
      expect(fixture.machineId).toEqual(undefined);
    });
  });
});

