import { Child } from '.';

export class Guest extends Child {
  GroupId: number;
  GradeGroupId: number;
  GuestSignin: boolean;
  YearGrade: number;

  static fromJson(json: any): Guest {
    let g = new Guest();
    if (json) {
      Object.assign(g, json);
    }
    return g;
  }
}
