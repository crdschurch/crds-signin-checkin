import { AvailableChildrenComponent } from './available-children.component';

let fixture: AvailableChildrenComponent;

describe('AvailableChildrenComponent', () => {
  describe('#toggleServingHours', () => {
    beforeEach(() => {
      fixture = new AvailableChildrenComponent(null, null, null, null);
      fixture.servingOneHour = true;
    });
    it('should activate a step and deactivate the other', () => {
      fixture.toggleServingHours(null, 2);
      expect(fixture.isServingOneHour).toBeFalsy();
      expect(fixture.isServingTwoHours).toBeTruthy();
      expect(fixture.numberEventsAttending).toEqual(2);
    });
    it('should allow you to toggle off an option', () => {
      fixture.toggleServingHours(null, 1);
      expect(fixture.isServingOneHour).toBeFalsy();
      expect(fixture.isServingTwoHours).toBeFalsy();
      expect(fixture.numberEventsAttending).toEqual(0);
    });
  });
});
