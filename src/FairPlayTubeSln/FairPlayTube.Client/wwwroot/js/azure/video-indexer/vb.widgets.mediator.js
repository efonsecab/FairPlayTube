
/**
 * AMP & Video Breakdown - Iframes Communication Mediator
 * Copyright (c) 2016 Microsoft
 * @author Ori Ziv
 * @version v0.8.80
 * @desc Incharge the communication between videobreakdown.com iframes.
 * @link https://www.videobreakdown.com
 * Note: This version has been slightly modified in order to work with FairPlayTube
 * Check original source here: https://docs.microsoft.com/en-us/azure/azure-video-analyzer/video-analyzer-for-media-docs/video-indexer-embed-widgets
 */

(function () {
    'use strict';

    // Jump to specific time from mesage payload
    function notifyWidgets(evt) {

        if (!evt) {
            return;
        }
        var origin = evt.origin || evt.originalEvent.origin;

        // Validate that event comes from videoindexer domain.
        if ((origin.indexOf(".videoindexer.ai") !== -1) && (evt.data.time !== undefined || evt.data.currentTime !== undefined || evt.data.language !== undefined)) {

            // Pass message to other iframe.
            if ('postMessage' in window) {
                var activeModals = window.document.getElementsByClassName("modal");
                var iframes = null;
                if (activeModals.length > 0) {
                    iframes = activeModals[0].getElementsByClassName("video-indexer-widget")
                }
                else {
                    iframes = window.document.getElementsByClassName('video-indexer-widget');
                }
                try {
                    for (var index = 0; index < iframes.length; index++) {
                        iframes[index].contentWindow.postMessage(evt.data, origin);
                    }
                } catch (error) {
                    throw error;
                }
            }
        }
    }

    function clearMessageEvent() {
        if (window.removeEventListener) {
            window.removeEventListener("message", notifyWidgets);
        }
    }

    // Listen to message events from breakdown iframes
    window.addEventListener("message", notifyWidgets, false);

    // Clear the event if window unloads
    window.onunload = clearMessageEvent;

}());