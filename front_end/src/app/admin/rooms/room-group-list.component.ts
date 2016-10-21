import { Component, OnInit } from '@angular/core';
import { AdminService } from '../admin.service';
import { Group } from './group';

@Component({
  templateUrl: 'room-group-list.component.html',
  styleUrls: ['room-group-list.component.scss'],
  providers: [ AdminService ]
})
export class RoomGroupListComponent implements OnInit {
  // groups: Group[];
  groups: any[];

  constructor( private adminService: AdminService) {
  }

  private getData(): void {
    // this.adminService.getEvents(this.currentWeekFilter.start, this.currentWeekFilter.end, this.site).subscribe(
    //   events => {this.events = events;},
    //   error => console.error(error)
    // );
    this.groups = [
      {
        "Id": 9014,
        "Name": "Nursery",
        "Selected": false,
        "SortOrder": 0,
        "TypeId": 102,
        "Ranges": [
          {
            "Id": 9020,
            "Name": "0-1",
            "Selected": false,
            "SortOrder": 0,
            "TypeId": 102
          },
          {
            "Id": 9021,
            "Name": "1-2",
            "Selected": false,
            "SortOrder": 1,
            "TypeId": 102
          },
          {
            "Id": 9022,
            "Name": "2-3",
            "Selected": false,
            "SortOrder": 2,
            "TypeId": 102
          },
          {
            "Id": 9023,
            "Name": "3-4",
            "Selected": false,
            "SortOrder": 3,
            "TypeId": 102
          },
          {
            "Id": 9024,
            "Name": "4-5",
            "Selected": false,
            "SortOrder": 4,
            "TypeId": 102
          },
          {
            "Id": 9025,
            "Name": "5-6",
            "Selected": false,
            "SortOrder": 5,
            "TypeId": 102
          },
          {
            "Id": 9026,
            "Name": "6-7",
            "Selected": false,
            "SortOrder": 6,
            "TypeId": 102
          },
          {
            "Id": 9027,
            "Name": "7-8",
            "Selected": false,
            "SortOrder": 7,
            "TypeId": 102
          },
          {
            "Id": 9028,
            "Name": "8-9",
            "Selected": false,
            "SortOrder": 8,
            "TypeId": 102
          },
          {
            "Id": 9029,
            "Name": "9-10",
            "Selected": false,
            "SortOrder": 9,
            "TypeId": 102
          },
          {
            "Id": 9030,
            "Name": "10-11",
            "Selected": false,
            "SortOrder": 10,
            "TypeId": 102
          },
          {
            "Id": 9031,
            "Name": "11-12",
            "Selected": false,
            "SortOrder": 11,
            "TypeId": 102
          }
        ]
      },
      {
        "Id": 9015,
        "Name": "First Year",
        "Ranges": [
          {
            "Id": 9002,
            "Name": "January",
            "Selected": false,
            "SortOrder": 0,
            "TypeId": 102
          },
          {
            "Id": 9003,
            "Name": "February",
            "Selected": false,
            "SortOrder": 1,
            "TypeId": 102
          },
          {
            "Id": 9004,
            "Name": "March",
            "Selected": false,
            "SortOrder": 2,
            "TypeId": 102
          },
          {
            "Id": 9005,
            "Name": "April",
            "Selected": false,
            "SortOrder": 3,
            "TypeId": 102
          },
          {
            "Id": 9006,
            "Name": "May",
            "Selected": false,
            "SortOrder": 4,
            "TypeId": 102
          },
          {
            "Id": 9007,
            "Name": "June",
            "Selected": false,
            "SortOrder": 5,
            "TypeId": 102
          },
          {
            "Id": 9008,
            "Name": "July",
            "Selected": false,
            "SortOrder": 6,
            "TypeId": 102
          },
          {
            "Id": 9009,
            "Name": "August",
            "Selected": false,
            "SortOrder": 7,
            "TypeId": 102
          },
          {
            "Id": 9010,
            "Name": "September",
            "Selected": false,
            "SortOrder": 8,
            "TypeId": 102
          },
          {
            "Id": 9011,
            "Name": "October",
            "Selected": false,
            "SortOrder": 9,
            "TypeId": 102
          },
          {
            "Id": 9012,
            "Name": "November",
            "Selected": false,
            "SortOrder": 10,
            "TypeId": 102
          },
          {
            "Id": 9013,
            "Name": "December",
            "Selected": false,
            "SortOrder": 11,
            "TypeId": 102
          }
        ],
        "Selected": false,
        "SortOrder": 1,
        "TypeId": 102
      },
      {
        "Id": 9016,
        "Name": "Second Year",
        "Ranges": [
          {
            "Id": 9002,
            "Name": "January",
            "Selected": false,
            "SortOrder": 0,
            "TypeId": 102
          },
          {
            "Id": 9003,
            "Name": "February",
            "Selected": false,
            "SortOrder": 1,
            "TypeId": 102
          },
          {
            "Id": 9004,
            "Name": "March",
            "Selected": false,
            "SortOrder": 2,
            "TypeId": 102
          },
          {
            "Id": 9005,
            "Name": "April",
            "Selected": false,
            "SortOrder": 3,
            "TypeId": 102
          },
          {
            "Id": 9006,
            "Name": "May",
            "Selected": false,
            "SortOrder": 4,
            "TypeId": 102
          },
          {
            "Id": 9007,
            "Name": "June",
            "Selected": false,
            "SortOrder": 5,
            "TypeId": 102
          },
          {
            "Id": 9008,
            "Name": "July",
            "Selected": false,
            "SortOrder": 6,
            "TypeId": 102
          },
          {
            "Id": 9009,
            "Name": "August",
            "Selected": false,
            "SortOrder": 7,
            "TypeId": 102
          },
          {
            "Id": 9010,
            "Name": "September",
            "Selected": false,
            "SortOrder": 8,
            "TypeId": 102
          },
          {
            "Id": 9011,
            "Name": "October",
            "Selected": false,
            "SortOrder": 9,
            "TypeId": 102
          },
          {
            "Id": 9012,
            "Name": "November",
            "Selected": false,
            "SortOrder": 10,
            "TypeId": 102
          },
          {
            "Id": 9013,
            "Name": "December",
            "Selected": false,
            "SortOrder": 11,
            "TypeId": 102
          }
        ],
        "Selected": false,
        "SortOrder": 2,
        "TypeId": 102
      },
      {
        "Id": 9017,
        "Name": "Third Year",
        "Ranges": [
          {
            "Id": 9002,
            "Name": "January",
            "Selected": false,
            "SortOrder": 0,
            "TypeId": 102
          },
          {
            "Id": 9003,
            "Name": "February",
            "Selected": false,
            "SortOrder": 1,
            "TypeId": 102
          },
          {
            "Id": 9004,
            "Name": "March",
            "Selected": false,
            "SortOrder": 2,
            "TypeId": 102
          },
          {
            "Id": 9005,
            "Name": "April",
            "Selected": false,
            "SortOrder": 3,
            "TypeId": 102
          },
          {
            "Id": 9006,
            "Name": "May",
            "Selected": false,
            "SortOrder": 4,
            "TypeId": 102
          },
          {
            "Id": 9007,
            "Name": "June",
            "Selected": false,
            "SortOrder": 5,
            "TypeId": 102
          },
          {
            "Id": 9008,
            "Name": "July",
            "Selected": false,
            "SortOrder": 6,
            "TypeId": 102
          },
          {
            "Id": 9009,
            "Name": "August",
            "Selected": false,
            "SortOrder": 7,
            "TypeId": 102
          },
          {
            "Id": 9010,
            "Name": "September",
            "Selected": false,
            "SortOrder": 8,
            "TypeId": 102
          },
          {
            "Id": 9011,
            "Name": "October",
            "Selected": false,
            "SortOrder": 9,
            "TypeId": 102
          },
          {
            "Id": 9012,
            "Name": "November",
            "Selected": false,
            "SortOrder": 10,
            "TypeId": 102
          },
          {
            "Id": 9013,
            "Name": "December",
            "Selected": false,
            "SortOrder": 11,
            "TypeId": 102
          }
        ],
        "Selected": false,
        "SortOrder": 3,
        "TypeId": 102
      },
      {
        "Id": 9018,
        "Name": "Fourth Year",
        "Ranges": [
          {
            "Id": 9002,
            "Name": "January",
            "Selected": false,
            "SortOrder": 0,
            "TypeId": 102
          },
          {
            "Id": 9003,
            "Name": "February",
            "Selected": false,
            "SortOrder": 1,
            "TypeId": 102
          },
          {
            "Id": 9004,
            "Name": "March",
            "Selected": false,
            "SortOrder": 2,
            "TypeId": 102
          },
          {
            "Id": 9005,
            "Name": "April",
            "Selected": false,
            "SortOrder": 3,
            "TypeId": 102
          },
          {
            "Id": 9006,
            "Name": "May",
            "Selected": false,
            "SortOrder": 4,
            "TypeId": 102
          },
          {
            "Id": 9007,
            "Name": "June",
            "Selected": false,
            "SortOrder": 5,
            "TypeId": 102
          },
          {
            "Id": 9008,
            "Name": "July",
            "Selected": false,
            "SortOrder": 6,
            "TypeId": 102
          },
          {
            "Id": 9009,
            "Name": "August",
            "Selected": false,
            "SortOrder": 7,
            "TypeId": 102
          },
          {
            "Id": 9010,
            "Name": "September",
            "Selected": false,
            "SortOrder": 8,
            "TypeId": 102
          },
          {
            "Id": 9011,
            "Name": "October",
            "Selected": false,
            "SortOrder": 9,
            "TypeId": 102
          },
          {
            "Id": 9012,
            "Name": "November",
            "Selected": false,
            "SortOrder": 10,
            "TypeId": 102
          },
          {
            "Id": 9013,
            "Name": "December",
            "Selected": false,
            "SortOrder": 11,
            "TypeId": 102
          }
        ],
        "Selected": false,
        "SortOrder": 4,
        "TypeId": 102
      },
      {
        "Id": 9019,
        "Name": "Fifth Year",
        "Ranges": [
          {
            "Id": 9002,
            "Name": "January",
            "Selected": false,
            "SortOrder": 0,
            "TypeId": 102
          },
          {
            "Id": 9003,
            "Name": "February",
            "Selected": false,
            "SortOrder": 1,
            "TypeId": 102
          },
          {
            "Id": 9004,
            "Name": "March",
            "Selected": false,
            "SortOrder": 2,
            "TypeId": 102
          },
          {
            "Id": 9005,
            "Name": "April",
            "Selected": false,
            "SortOrder": 3,
            "TypeId": 102
          },
          {
            "Id": 9006,
            "Name": "May",
            "Selected": false,
            "SortOrder": 4,
            "TypeId": 102
          },
          {
            "Id": 9007,
            "Name": "June",
            "Selected": false,
            "SortOrder": 5,
            "TypeId": 102
          },
          {
            "Id": 9008,
            "Name": "July",
            "Selected": false,
            "SortOrder": 6,
            "TypeId": 102
          },
          {
            "Id": 9009,
            "Name": "August",
            "Selected": false,
            "SortOrder": 7,
            "TypeId": 102
          },
          {
            "Id": 9010,
            "Name": "September",
            "Selected": false,
            "SortOrder": 8,
            "TypeId": 102
          },
          {
            "Id": 9011,
            "Name": "October",
            "Selected": false,
            "SortOrder": 9,
            "TypeId": 102
          },
          {
            "Id": 9012,
            "Name": "November",
            "Selected": false,
            "SortOrder": 10,
            "TypeId": 102
          },
          {
            "Id": 9013,
            "Name": "December",
            "Selected": false,
            "SortOrder": 11,
            "TypeId": 102
          }
        ],
        "Selected": false,
        "SortOrder": 5,
        "TypeId": 102
      },
      {
        "Id": 9032,
        "Name": "Kindergarten",
        "Ranges": null,
        "Selected": false,
        "SortOrder": 5,
        "TypeId": 104
      },
      {
        "Id": 9033,
        "Name": "First Grade",
        "Ranges": null,
        "Selected": false,
        "SortOrder": 6,
        "TypeId": 104
      },
      {
        "Id": 9034,
        "Name": "Second Grade",
        "Ranges": null,
        "Selected": false,
        "SortOrder": 7,
        "TypeId": 104
      },
      {
        "Id": 9035,
        "Name": "Third Grade",
        "Ranges": null,
        "Selected": false,
        "SortOrder": 8,
        "TypeId": 104
      },
      {
        "Id": 9036,
        "Name": "Fourth Grade",
        "Ranges": null,
        "Selected": false,
        "SortOrder": 9,
        "TypeId": 104
      },
      {
        "Id": 9037,
        "Name": "Fifth Grade",
        "Ranges": null,
        "Selected": false,
        "SortOrder": 10,
        "TypeId": 104
      },
      {
        "Id": 9038,
        "Name": "Sixth Grade",
        "Ranges": null,
        "Selected": false,
        "SortOrder": 11,
        "TypeId": 104
      },
      {
        "Id": 9039,
        "Name": "CSM",
        "Ranges": null,
        "Selected": false,
        "SortOrder": 12,
        "TypeId": 104
      }
    ];

  }

  ngOnInit() {
    this.getData()
  }
}
