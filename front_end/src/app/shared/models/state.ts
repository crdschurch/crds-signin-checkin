export class State {
  StateId: number;
  StateName: string;
  StateAbbreviation: string;


  static fromJson(json: any): State {
    let s = new State();
    s.StateId = json.StateId;
    s.StateName = json.StateName;
    s.StateAbbreviation = json.StateAbbreviation;

    return s;
  }
}
