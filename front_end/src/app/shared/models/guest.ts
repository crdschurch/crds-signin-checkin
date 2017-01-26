import { Child } from '.';

export class Guest extends Child {
  GroupId: number;
  GradeGroupId: number;
  GuestSignin: boolean;
  YearGrade: number;
  LocalId: number;

  static fromJson(json: any): Guest {
    let g = new Guest();
    if (json) {
      Object.assign(g, json);
    }
    return g;
  }

  uniqueIdentifier() {
    return this.LocalId;
  }

  constructor() {
    super();
    // generate an id so inputs can be attached to label in html
    this.LocalId = Math.floor(Math.random() * 1000000000);
  }
}
