import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';

import { Http, Headers, RequestOptions, Response } from '@angular/http';
import { Toast, BodyOutputType } from 'angular2-toaster/angular2-toaster';
import { ContentBlock } from '../models/contentBlock';

@Injectable()
export class ContentService {

  private contentBlockTitle: string;
  contentBlocks: ContentBlock[];

  constructor(private http: Http) {
    
  }

  loadData() {
    // call for each type of content block used in the app
    this.getContentBlocks('main').then(contentBlocks => {
      this.contentBlocks = contentBlocks;

      this.getContentBlocks('common').then(contentBlocks => {
        this.contentBlocks = this.contentBlocks.concat(contentBlocks);
      });
      
    });
  }

  getContentBlocks (categoryName) {
      const url = `${process.env.CRDS_CMS_ENDPOINT}api/contentblock?category=` + categoryName;
      return this.http.get(url).toPromise()
                    .then(res => {return res.json().contentblocks})
                    .catch(this.handleError);
  }

  getToastContent (contentBlockTitle): Promise<any> {
    this.contentBlockTitle = contentBlockTitle;
    if (this.contentBlocks === undefined) {
      return this.getContentBlocks('main').then(contentBlocks => {
        this.contentBlocks = contentBlocks;

        return this.getContentBlocks('common').then(contentBlocks => {
          this.contentBlocks = this.contentBlocks.concat(contentBlocks);
          return this.displayToast(this.contentBlockTitle);
        });
    
      });
    } else {
      return Promise.resolve(this.displayToast(this.contentBlockTitle));
    }
  }

  displayToast(contentBlockTitle)
  {
      var contentBlock = this.contentBlocks.find(x => x.title === contentBlockTitle);

      var toast : Toast = {
      type: contentBlock.type,
      body: contentBlock.content,
      bodyOutputType: BodyOutputType.TrustedHtml
    };

    return toast;
  }
  
  private handleError (error: any) {
    console.error(error);
    return Observable.throw(error.json().error || 'Server error');
  }

}