import { Child, Contact, Event } from '.';

export class EventParticipants {
  Contacts: Array<Contact>;
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
    eventParticipants.Contacts = Array.isArray(json.Contacts) ? (<Array<any>>(json.Contacts)).map(c => Contact.fromJson(c)) : [];
    return eventParticipants;
  }

  public hasSelectedParticipants() {
    return this.Participants && this.Participants.length > 0 && this.Participants.find(p => p.selected()) !== undefined;
  }
}
