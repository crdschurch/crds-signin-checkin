import { NewChild } from '.';

export class Guest extends NewChild {
  GroupId: number;
  GradeGroupId: number;
  GuestSignin: boolean;

  static fromJson(json: any): Guest {
    let g = new Guest();
    if (json) {
      Object.assign(g, json);
    }
    return g;
  }
}
