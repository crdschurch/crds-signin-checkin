import {Pipe} from '@angular/core';

@Pipe({
  name: 'ClosedRoomPipe'
})

export class ClosedRoomPipe {
  transform(value, allowSignIn: boolean) {
    return value.filter(room => {

      if (allowSignIn === true) {
        return room.AllowSignIn === allowSignIn;
      } else {
        return room.AllowSignIn !== undefined;
      }
      
    });
  }
}