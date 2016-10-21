import { NumberPadComponent } from './number-pad.component';

let fixture: NumberPadComponent;
let addNumberSpy: any;
let deleteNumberSpy: any;
let clearNumberSpy: any;

describe('NumberPadComponent', () => {

  beforeEach(() => {
    fixture = new NumberPadComponent();
    addNumberSpy = spyOn(fixture.addNumber, 'emit').and.returnValue(true);
    deleteNumberSpy = spyOn(fixture.deleteNumber, 'emit').and.returnValue(true);
    clearNumberSpy = spyOn(fixture.clearNumber, 'emit').and.returnValue(true);
  });

  describe('#setNumber', () => {
    it('should return number', () => {
      fixture.setNumber(1);
      expect(addNumberSpy.calls.any()).toBe(true, 'addNumber.emit called');
    });
  });

  describe('#delete', () => {
    it('should return delete', () => {
      fixture.delete();
      expect(deleteNumberSpy.calls.any()).toBe(true, 'deleteNumber.emit called');
    });
  });

  describe('#clear', () => {
    it('should return clear', () => {
      fixture.clear();
      expect(clearNumberSpy.calls.any()).toBe(true, 'clearNumber.emit called');
    });
  });
});
