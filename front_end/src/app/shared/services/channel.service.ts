import {Injectable, Inject, ApplicationRef} from '@angular/core';
import {Subject} from 'rxjs/Subject';
import {Observable} from 'rxjs/Observable';

/**
 * Taken from: https://github.com/sstorie/experiments/tree/master/angular2-signalr
 *
 * When SignalR runs it will add functions to the global $ variable
 * that you use to create connections to the hub. However, in this
 * class we won't want to depend on any global variables, so this
 * class provides an abstraction away from using $ directly in here.
 */
export class SignalrWindow extends Window {
    $: any;
}

export enum ConnectionState {
    Connecting = 1,
    Connected = 2,
    Reconnecting = 3,
    Disconnected = 4
}

export class ChannelConfig {
    url: string;
    hubName: string;
    channel: string;
}

export class ChannelEvent {
    Name: string;
    ChannelName: string;
    Timestamp: Date;
    Data: any;
    Json: string;

    constructor() {
        this.Timestamp = new Date();
    }
}

class ChannelSubject {
    channel: string;
    subject: Subject<ChannelEvent>;
}

/**
 * ChannelService is a wrapper around the functionality that SignalR
 * provides to expose the ideas of channels and events. With this service
 * you can subscribe to specific channels (or groups in signalr speak) and
 * use observables to react to specific events sent out on those channels.
 */
@Injectable()
export class ChannelService {

    /**
     * starting$ is an observable available to know if the signalr
     * connection is ready or not. On a successful connection this
     * stream will emit a value.
     */
    starting$: Observable<any>;

    /**
     * connectionState$ provides the current state of the underlying
     * connection as an observable stream.
     */
    connectionState$: Observable<ConnectionState>;

    /**
     * error$ provides a stream of any error messages that occur on the
     * SignalR connection
     */
    error$: Observable<string>;

    public networkError = false;
    public timeToRefresh = 5000;
    public timeLeft = 5;
    // These are used to feed the public observables
    //
    private connectionStateSubject = new Subject<ConnectionState>();
    private startingSubject = new Subject<any>();
    private errorSubject = new Subject<any>();

    // These are used to track the internal SignalR state
    //
    private hubConnection: any;
    private hubProxy: any;
    private isConnected = false;

    // An internal array to track what channel subscriptions exist
    //
    private subjects = new Array<ChannelSubject>();

    constructor(
        @Inject(SignalrWindow) private window: SignalrWindow,
        @Inject('channel.config') private channelConfig: ChannelConfig,
        private ref: ApplicationRef
    ) {
        if (this.window.$ === undefined || this.window.$.hubConnection === undefined) {
            throw new Error(
              'The variable "$" or the .hubConnection() function are not defined...please ' +
              'check the SignalR scripts have been loaded properly');
        }

        this.timeLeft = this.timeToRefresh/1000;
        // Set up our observables
        //
        this.connectionState$ = this.connectionStateSubject.asObservable();
        this.error$ = this.errorSubject.asObservable();
        this.starting$ = this.startingSubject.asObservable();

        this.hubConnection = this.window.$.hubConnection();
        this.hubConnection.url = channelConfig.url;
        this.hubProxy = this.hubConnection.createHubProxy(channelConfig.hubName);

        // Define handlers for the connection state events
        //
        this.hubConnection.stateChanged((state: any) => {
            let newState = ConnectionState.Connecting;
            switch (state.newState) {
                case this.window.$.signalR.connectionState.connecting:
                    newState = ConnectionState.Connecting;
                    break;
                case this.window.$.signalR.connectionState.connected:
                    newState = ConnectionState.Connected;
                    break;
                case this.window.$.signalR.connectionState.reconnecting:
                    newState = ConnectionState.Reconnecting;
                    break;
                case this.window.$.signalR.connectionState.disconnected:
                    newState = ConnectionState.Disconnected;
                    break;
            }

            // Push the new state on our subject
            //
            this.connectionStateSubject.next(newState);
        });

        // Define handlers for any errors
        //
        this.hubConnection.error((error: any) => {
            // Push the error on our subject
            //
            this.errorSubject.next(error);
        });

        this.hubProxy.on('onEvent', (channel: string, ev: ChannelEvent) => {

            // This method acts like a broker for incoming messages. We
            //  check the interal array of subjects to see if one exists
            //  for the channel this came in on, and then emit the event
            //  on it. Otherwise we ignore the message.
            //
            let channelSub = this.subjects.find((x: ChannelSubject) => {
                return x.channel === channel;
            }) as ChannelSubject;

            // If we found a subject then emit the event on it
            //
            if (channelSub !== undefined) {
                return channelSub.subject.next(ev);
            }
        });

        // Let's wire up to the signalr observables
        this.connectionState$
            .map((state: ConnectionState) => { return ConnectionState[state]; });

        this.error$.subscribe(
            (error: any) => {
              this.networkErrorResponse();
            }
        );

        // Wire up a handler for the starting$ observable to log the
        //  success/fail result
        this.starting$.subscribe(
            () => { console.log('signalr service has been started'); },
            () => { console.warn('signalr service failed to start!'); }
        );
    }

    /**
     * Start the SignalR connection. The starting$ stream will emit an
     * event if the connection is established, otherwise it will emit an
     * error.
     */
    start(): void {
        // Now we only want the connection started once, so we have a special
        //  starting$ observable that clients can subscribe to know know if
        //  if the startup sequence is done.
        //
        // If we just mapped the start() promise to an observable, then any time
        //  a client subscried to it the start sequence would be triggered
        //  again since it's a cold observable.
        //
        this.hubConnection.start()
            .done(() => {
                this.isConnected = true;
                this.startingSubject.next();
            })
            .fail((error: any) => {
              this.startingSubject.error(error);
              this.networkErrorResponse();
            });
    }

    stop(): void {
        this.hubConnection.stop();
    }

    unsub(channelName: string) {
      let channelSub = this.subjects.find((x: ChannelSubject) => {
          return x.channel === channelName;
      }) as ChannelSubject;
      const index = this.subjects.findIndex((x: ChannelSubject) => {
          return x.channel === channelName;
      });
      this.subjects.splice(index, 1);
      channelSub.subject.unsubscribe();
      console.log('Unsubscribed from', channelSub);
      console.log('Subscribed channels:', this.subjects);
    }

    unsubAll(channelPrefix: string) {
      // get all channels that match string
      const allChannels = this.subjects.filter(x => x.channel.includes(channelPrefix));
      for (let subscribedChannel of allChannels) {
        this.unsub(subscribedChannel.channel);
      }
    }

    /**
     * Get an observable that will contain the data associated with a specific
     * channel
     * */
    sub(channel: string): Observable<ChannelEvent> {
        // Try to find an observable that we already created for the requested
        //  channel

        let channelSub = this.subjects.find((x: ChannelSubject) => {
            return x.channel === channel;
        }) as ChannelSubject;

        // If we already have one for this event, then just return it
        //
        if (channelSub !== undefined) {
            console.log(`Found existing observable for ${channel} channel`);
            return channelSub.subject.asObservable();
        }

        //
        // If we're here then we don't already have the observable to provide the
        //  caller, so we need to call the server method to join the channel
        //  and then create an observable that the caller can use to received
        //  messages.
        //

        // Now we just create our internal object so we can track this subject
        //  in case someone else wants it too
        //
        channelSub = new ChannelSubject();
        channelSub.channel = channel;
        channelSub.subject = new Subject<ChannelEvent>();
        this.subjects.push(channelSub);

        // Now SignalR is asynchronous, so we need to ensure the connection is
        //  established before we call any server methods. So we'll subscribe to
        //  the starting$ stream since that won't emit a value until the connection
        //  is ready
        //

        // TODO this should really be binded to this.start in some way
        // because this.channelService.start() in app component
        // could possibly have not started the connection (hubProxy)
        // by this point
        if (this.isConnected) {
          this.subscribeToHubProxy(channel, channelSub);
        } else {
          this.starting$.subscribe((resp) => {
            this.subscribeToHubProxy(channel, channelSub);
          });
        }
        return channelSub.subject.asObservable();
    }

    private subscribeToHubProxy(channel, channelSub) {
      this.hubProxy.invoke('Subscribe', channel)
          .done(() => {
              console.log(`Successfully subscribed to ${channel} channel`);
              console.log('Subscribed channels:', this.subjects);
          })
          .fail((error: any) => {
            channelSub.subject.error(error);
            debugger
            this.networkErrorResponse();
          });
    }

    /** publish provides a way for calles to emit events on any channel. In a
     * production app the server would ensure that only authorized clients can
     * actually emit the message, but here we're not concerned about that.
     */
    publish(ev: ChannelEvent): void {
        this.hubProxy.invoke('Publish', ev);
    }

    private networkErrorResponse() {
      let that = this;

      if (!that.networkError) {
        that.networkError = true;

        setTimeout(() => { that.window.location.reload(); }, that.timeToRefresh);

        setInterval(() => {
          that.timeLeft = that.timeLeft - 1;
          this.ref.tick();
        }, 1000);
      }
    }

}
