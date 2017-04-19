import { ServingToggleComponent } from './serving-toggle.component';

let fixture: ServingToggleComponent;
let fakeModal = { show: {}, hide: {} };

describe('AvailableChildrenComponent', () => {
  beforeEach(() => {
    fixture = new ServingToggleComponent();
    spyOn(fakeModal, 'show').and.callFake(() => {});
    spyOn(fakeModal, 'hide').and.callFake(() => {});
  });
  describe('#toggleServicesClick', () => {
    it('should open modal if no services selected', () => {
      fixture.toggleServicesClick(fakeModal);
      expect(fakeModal.show).toHaveBeenCalled();
    });
    it('should toggle off if services selected', () => {
      fixture.toggleServingHours(null, 1);
      fixture.toggleServicesClick(fakeModal);
    });
  });
  describe('#toggleServingHours', () => {
    it('should allow you to select 1 hour', () => {
      fixture.toggleServingHours(fakeModal, 1);
      expect(fixture.isServingOneHour).toBeTruthy();
      expect(fixture.isServingTwoHours).toBeFalsy();
      expect(fixture.isServing).toBeTruthy();
      expect(fixture.numberEventsAttending).toEqual(1);
      expect(fakeModal.hide).toHaveBeenCalled();
    });
    it('should allow you to select 2 hours', () => {
      fixture.toggleServingHours(fakeModal, 2);
      expect(fixture.isServingTwoHours).toBeTruthy();
      expect(fixture.isServingOneHour).toBeFalsy();
      expect(fixture.isServing).toBeTruthy();
      expect(fixture.numberEventsAttending).toEqual(2);
      expect(fakeModal.hide).toHaveBeenCalled();
    });
  });
});
