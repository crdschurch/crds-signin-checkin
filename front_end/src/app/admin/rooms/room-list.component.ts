import { Component } from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { AdminService } from '../admin.service';
import { Room } from './room';

@Component({
  // selector: 'rooms',
  templateUrl: 'room-list.component.html',
  styleUrls: ['room-list.component.scss'],
  providers: [ AdminService ]
})
export class RoomListComponent {
  rooms: Room[];

  constructor(
    private route: ActivatedRoute,
    private adminService: AdminService,
    private service: AdminService) {}

  private getData(): void {
    const eventId = this.route.snapshot.params['eventId'];
    this.adminService.getRooms(eventId).subscribe(
      (rooms: Room[]) => {this.rooms = rooms},
      (error: any) => console.error(error)
    );
  }
  ngOnInit(): void {
    this.getData();
  }

  removeVolunteer(room: Room): void {
    if(room.Volunteers > 0) {
      room.Volunteers--
      this.adminService.updateRoom(room.EventId, room.RoomId, room).subscribe((room: Room) => { room = room },
        (error: any) => {});
    }
  }

  addVolunteer(room: Room): void {
    room.Volunteers++
    this.adminService.updateRoom(room.EventId, room.RoomId, room).subscribe((room: Room) => { room = room },
      (error: any) => {});
  }

  removeCapacity(room: Room): void {
    if(room.Capacity > 0) {
      room.Capacity--
      this.adminService.updateRoom(room.EventId, room.RoomId, room).subscribe((room: Room) => { room = room },
        (error: any) => {});
    }
  }

  addCapacity(room: Room): void {
    room.Capacity++
    this.adminService.updateRoom(room.EventId, room.RoomId, room).subscribe((room: Room) => {},
      (error: any) => {});
  }

  toggleAllowSignin(room: Room): void {
    room.AllowSignIn = !room.AllowSignIn
    this.adminService.updateRoom(room.EventId, room.RoomId, room).subscribe((room: Room) => { room = room },
      (error: any) => {});
  }

}
