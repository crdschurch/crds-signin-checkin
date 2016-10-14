import { Component } from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { AdminService } from '../admin.service';
import { Room } from '../models/room';

@Component({
  selector: 'rooms',
  templateUrl: 'rooms.component.html',
  styleUrls: ['rooms.component.scss'],
  providers: [ AdminService ]
})
export class RoomsComponent {
  rooms: Room[];

  constructor(
    private route: ActivatedRoute,
    private adminService: AdminService,
    private service: AdminService) {}

  private getData(): void {
    const eventId = this.route.snapshot.params['eventId'];
    this.adminService.getRooms(eventId).subscribe(
      rooms => {this.rooms = rooms},
      error => console.error(error)
    );
  }
  ngOnInit(): void {
    this.getData();
  }

  removeVolunteer(room: any): void {
    if(room.Volunteers > 0)
      room.Volunteers--
    true;
  }

  addVolunteer(room: any): void {
    room.Volunteers++
    true;
  }

  removeCapacity(room: any): void {
    if(room.Capacity > 0)
      room.Capacity--
    true;
  }

  addCapacity(room: any): void {
    room.Capacity++
    true;
  }

}
