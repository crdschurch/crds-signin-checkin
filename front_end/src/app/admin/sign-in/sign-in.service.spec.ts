import { SignInService } from './sign-in.service';
import { HttpClientService } from '../../shared/services/http-client.service';
import { Response } from '@angular/http';
import { Observable } from 'rxjs';

describe('SignInService', () => {
  let fixture: SignInService;
  let httpClientService: HttpClientService;
  let responseObject: Response;
  let response: Observable<Response>;
  let expectedBody: any;

  beforeEach(() => {
    httpClientService = jasmine.createSpyObj<HttpClientService>('httpClientService', ['post']);
    responseObject = jasmine.createSpyObj('response', ['json']);
    response = Observable.of(responseObject);

    fixture = new SignInService(httpClientService);
    expectedBody = { username: 'test@test.com', password: 'password123' };
  });

  describe('when logging in', () => {
    describe('with successful credentials', () => {
      beforeEach(() => {
        (<jasmine.Spy>httpClientService.post).and.returnValue(response);
      });

      it('should successfully login a user', () => {
        fixture.logIn(expectedBody.username, expectedBody.password).subscribe(res => {
          expect(res).toBe(responseObject);
          expect(httpClientService.post).toHaveBeenCalledWith(
            `${process.env.ECHECK_API_ENDPOINT}/authenticate`,
            JSON.stringify(expectedBody)
          );
        });
      });
    });

    describe('with bad credentials', () => {
      beforeEach(() => {
        (<jasmine.Spy>httpClientService.post).and.returnValue(Observable.throw('error logging in'));
      });

      it('should not login a user', () => {
        fixture.logIn(expectedBody.username, expectedBody.password).subscribe(() => {
        }, error => {
          expect(error).toEqual('error logging in');
          expect(httpClientService.post).toHaveBeenCalledWith(
            `${process.env.ECHECK_API_ENDPOINT}/authenticate`,
            JSON.stringify(expectedBody)
          );
        });
      });
    });
  });
});
