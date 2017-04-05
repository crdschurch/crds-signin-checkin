export class Event {
  static TYPE = {
    ADVENTURE_CLUB: 20
  };
  EventId: number;
  EventTitle: string;
  EventStartDate: string;
  EventTypeId: number;
  EventSite: string;
  IsCurrentEvent: boolean;
  EventSiteId: number;
  ParentEventId: number;

  static fromJson(json: any): Event {
    let e = new Event();
    e.EventId = json.EventId;
    e.EventTitle = json.EventTitle;
    e.EventStartDate = json.EventStartDate;
    e.EventTypeId = json.EventTypeId;
    e.EventSite = json.EventSite;
    e.IsCurrentEvent = json.IsCurrentEvent;
    e.EventSiteId = json.EventSiteId;
    e.ParentEventId = json.ParentEventId;
    return e;
  }

  static fromJsons(jsons: any): Event[] {
    let events: Event[] = [];
    for (let json of jsons) {
        events.push(Event.fromJson(json));
    }
    return events;
  }

  public isAdventureClub() {
    return this.EventTypeId === Event.TYPE.ADVENTURE_CLUB;
  }

  get type() {
    switch (this.EventTypeId) {
      case 81:
        return 'Student Ministry';
      case 243:
        return 'Childcare';
      default:
        return 'Service';
    }
  }
}
