import { SetupComponent } from './setup.component';
import { Observable } from 'rxjs/Observable';

let fixture: SetupComponent;

let setupServiceStub: any = {
  setMachineIdConfigCookie: {},
  getMachineIdConfigCookie: {}
};

let rootServiceStub: any = {
  announceEvent(): void {
  }
};

fdescribe('SetupComponent', () => {
  describe('#ngOnInit', () => {
    let setMachineIdConfigCookieSpy;
    let getMachineIdConfigCookieSpy;

    beforeEach(() => {
      setMachineIdConfigCookieSpy = spyOn(setupServiceStub, 'setMachineIdConfigCookie');
      getMachineIdConfigCookieSpy = spyOn(setupServiceStub, 'getMachineIdConfigCookie').and.returnValue(undefined);
    });

    it('should get the param values and have no machine id', () => {

      let routerStub: any = {
        params: Observable.of({'error': false})
      };

      fixture = new SetupComponent(setupServiceStub, routerStub, rootServiceStub);
      fixture.ngOnInit();

      expect(fixture.isError).toBeFalsy();
      expect(fixture.machineId).toEqual(undefined);
      expect(setMachineIdConfigCookieSpy).not.toHaveBeenCalled();
      expect(getMachineIdConfigCookieSpy).toHaveBeenCalled();
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
      expect(setMachineIdConfigCookieSpy).toHaveBeenCalledWith('xyz');
      expect(getMachineIdConfigCookieSpy).not.toHaveBeenCalled();
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
      expect(fixture.machineId).not.toBeDefined();
    });
  });
});
