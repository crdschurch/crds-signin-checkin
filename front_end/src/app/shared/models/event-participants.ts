import { Child, Event } from '.';

export class EventParticipants {
  CurrentEvent: Event;
  Participants: Array<Child>;

  static fromJson(json: any): EventParticipants {
    if (!json) {
      return new EventParticipants();
    }

    let eventParticipants = new EventParticipants();
    eventParticipants.CurrentEvent = Event.fromJson(json.CurrentEvent);
    eventParticipants.Participants = [];
    for (let p of json.Participants) {
      eventParticipants.Participants.push(Child.fromJson(p));
    }
    return eventParticipants;
  }

  public hasSelectedParticipants() {
    return this.Participants && this.Participants.length > 0 && this.Participants.find(p => p.selected()) !== undefined;
  }
}
