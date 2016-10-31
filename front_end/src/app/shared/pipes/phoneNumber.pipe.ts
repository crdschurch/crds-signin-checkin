import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
    name: 'phoneNumber'
})
export class PhoneNumberPipe implements PipeTransform {
  transform(val: string, justDashes = false) {
    if (!val) {
      return '';
    }

    switch (val.length) {
      case 1:
      case 2:
      case 3:
        return justDashes ? `${val}-` : `(${val}) - `;

      case 4:
      case 5:
      case 6:
        return justDashes ?
                `${val.slice(0, 3)}-${val.slice(3)}-` :
                `(${val.slice(0, 3)}) ${val.slice(3)}-`;

      case 7:
      case 8:
      case 9:
      case 10:
        return justDashes ?
                `${val.slice(0, 3)}-${val.slice(3, 6)}-${val.slice(6)}` :
                `(${val.slice(0, 3)}) ${val.slice(3, 6)}-${val.slice(6)}`;
    }
  }
}
