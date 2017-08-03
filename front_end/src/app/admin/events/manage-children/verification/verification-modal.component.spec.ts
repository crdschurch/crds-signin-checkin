import { VerificationModalComponent } from './verification-modal.component';
import { Child } from '../../../../shared/models';

let fixture: VerificationModalComponent;
let fakeModal = { show: {}, hide: {} };

describe('VerificationModalComponent', () => {
  beforeEach(() => {
    spyOn(fakeModal, 'show').and.callFake(() => {});
    spyOn(fakeModal, 'hide').and.callFake(() => {});
    fixture = new VerificationModalComponent();
  });
  describe('#showVerificationModal', () => {
    beforeEach(() => {
      fakeModal = { show: {}, hide: {} };
      spyOn(fakeModal, 'show').and.callFake(() => {});
      spyOn(fakeModal, 'hide').and.callFake(() => {});
    });
    it('no parents', () => {
      fixture.showVerificationModal(fakeModal);
      expect(fixture.showModal).toBeTruthy();
      expect(fixture.parent1).toEqual(undefined);
      expect(fixture.parent2).toEqual(undefined);
    });
  });
  describe('#ngOnInit', () => {
    it('should show button to reprint', () => {
      fixture.ngOnInit();
      expect(fixture.showVerificationOption).toBeTruthy();
    });
  });
});
