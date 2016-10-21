import { SearchComponent } from './search.component';

let fixture: SearchComponent;
let routerStub: any = {
  navigate(): void {
  }
};

describe('SearchComponent', () => {

  beforeEach(() => {
    fixture = new SearchComponent(routerStub);
  });

  describe('#constructor', () => {
    it('should intialize to empty phone number', () => {
      expect(fixture.phoneNumber).toEqual('');
    });
  });

  describe('#setPhoneNumber', () => {
    it('should add a number to the current phone number', () => {
      fixture.setPhoneNumber('1');
      expect(fixture.phoneNumber).toEqual('1');

      fixture.setPhoneNumber('2');
      fixture.setPhoneNumber('3');
      expect(fixture.phoneNumber).toEqual('123');
    });
  });

  describe('#delete', () => {
    it('should not make null or undefined if deleting a blank string', () => {
      fixture.delete();
      expect(fixture.phoneNumber).toEqual('');
    });

    it('should delete the last character', () => {
      fixture.setPhoneNumber('1');
      expect(fixture.phoneNumber).toEqual('1');

      fixture.delete();
      expect(fixture.phoneNumber).toEqual('');

      fixture.setPhoneNumber('1');
      fixture.setPhoneNumber('2');
      fixture.setPhoneNumber('3');
      expect(fixture.phoneNumber).toEqual('123');

      fixture.delete();
      expect(fixture.phoneNumber).toEqual('12');
    });
  });

  describe('#clear', () => {
    it('should not make null or undefined if clearing a blank string', () => {
      fixture.clear();
      expect(fixture.phoneNumber).toEqual('');
    });

    it('should clear if any values', () => {
      fixture.setPhoneNumber('1');
      expect(fixture.phoneNumber).toEqual('1');

      fixture.clear();
      expect(fixture.phoneNumber).toEqual('');

      fixture.setPhoneNumber('1');
      fixture.setPhoneNumber('2');
      fixture.setPhoneNumber('3');
      expect(fixture.phoneNumber).toEqual('123');

      fixture.clear();
      expect(fixture.phoneNumber).toEqual('');
    });
  });

  describe('#next', () => {
    it('should set to error if phone number is not 10 characters', () => {
      fixture.setPhoneNumber('1');
      fixture.setPhoneNumber('2');
      fixture.setPhoneNumber('3');
      fixture.next();
      expect(fixture.error).toBeFalsy;
    });

    it('should not set an error if more than 10 characters', () => {
      fixture.setPhoneNumber('1');
      fixture.setPhoneNumber('2');
      fixture.setPhoneNumber('3');
      fixture.next();
      expect(fixture.error).toBeTruthy;
    });
  });
});
