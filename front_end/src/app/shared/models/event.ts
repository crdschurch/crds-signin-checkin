export class Event {
  EventId: number;
  EventTitle: string;
  EventStartDate: string;
  EventTypeId: number;
  EventSite: string;
  IsCurrentEvent: boolean;
  EventSiteId: number;
  ParentEventId: number;

  private TYPE = {
    ADVENTURE_CLUB: 20
  };

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
    return this.EventTypeId === this.TYPE.ADVENTURE_CLUB;
  }

  constructor() {
  }
}
