# jcPF
Universal NDIS Driver for Packet Filtering and Machine Learning

## First Pass
Originally I was going to write an NDIS driver, but wanted to deep dive into packet parsing on the wire.  Using the Pcap.net library I'm hoping to do the following:
* Capture and store packets in a SQLite Database
* Provide analysis on most commonly hit ip addresses, packet lengths
* Provide analysis of anomolies

## Second Pass
Write the NDIS driver that performs the same analysis done in the first pass
