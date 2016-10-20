import { Component, Input } from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { FormControl, FormGroup } from '@angular/forms';
import { AdminService } from '../admin.service';
import { Room } from './room';

@Component({
  selector: '.room',
  templateUrl: 'room.component.html',
  styleUrls: ['room.component.scss']
})
export class RoomComponent {
  @Input() room: Room;

  coolForm = new FormGroup({
    Volunteers: new FormControl(),
    Capacity: new FormControl()
  });

  constructor(
    private adminService: AdminService) {
     this.coolForm.valueChanges
        .debounceTime(1000)
        .subscribe(props => {
          this.adminService.updateRoom(this.room.EventId, this.room.RoomId, this.room).subscribe(val => {})
        });
    }

  add(field): void { this.coolForm.controls[field].setValue(this.room[field]++); }
  remove(field): void { this.coolForm.controls[field].setValue(this.room[field]--); }
  // removeVolunteer(): void { this.coolForm.controls["volunteers"].setValue(this.room.Volunteers--); }
  // addCapacity(): void { this.coolForm.controls["capacity"].setValue(this.room.Capacity++); }
  // removeCapacity(): void { this.coolForm.controls["capacity"].setValue(this.room.Capacity--); }
}
