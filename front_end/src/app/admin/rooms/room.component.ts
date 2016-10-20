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
  term = new FormControl();

  constructor(
    private adminService: AdminService) {
      this.term.valueChanges
         .debounceTime(400)
         .subscribe(term => {
           console.log("updateRoom", this)
           this.adminService.updateRoom(this.room.EventId, this.room.RoomId, this.room).then(val => {
             console.log("val", val);
           })
         });
    }
  //
  // private getData(): void {
  //   const eventId = this.route.snapshot.params['eventId'];
  //   const roomId = this.route.snapshot.params['roomId'];
  //   this.adminService.getRoom(eventId, roomId).subscribe(
  //     room => {this.room = room},
  //     error => console.error(error)
  //   );
  // }
  ngOnInit(): void {
    // this.getData();
    console.log("room component oninit", this)
  }

  addVolunteer(): void {
    console.log("addVolunteer", this.room)
    // room.Volunteers++
    // this.adminService.updateRoom(room.EventId, room.RoomId, room).subscribe((room: Room) => { room = room },
    //   (error: any) => {});
  }
}
