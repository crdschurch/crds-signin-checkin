import { Component, Input } from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { FormControl } from '@angular/forms';
import { AdminService } from '../admin.service';
import { Room } from './room';

@Component({
  selector: '.room',
  templateUrl: 'room.component.html',
  styleUrls: ['room.component.scss']
})
export class RoomComponent {
  @Input() room: Room;
  volunteers = new FormControl();

  constructor(
    private adminService: AdminService) {
     this.volunteers.valueChanges
        .debounceTime(1000)
        .subscribe(props => {
          this.adminService.updateRoom(this.room.EventId, this.room.RoomId, this.room).subscribe(val => {
            console.log("val", val);
          })
        });
    }

  ngOnInit(): void {
    console.log("room component oninit", this)
  }

  addVolunteer(): void { this.volunteers.setValue(this.room.Volunteers++) }
  removeVolunteer(): void { this.volunteers.setValue(this.room.Volunteers--) }
  // addCapacity(): void { this.props.setValue(this.room.Capacity++) }
  // removeCapacity(): void { this.props.setValue(this.room.Capacity--) }
}
