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
  addVolunteer(room: any): void {
    console.log("addVolunteer", room);
    true;
  }
}
