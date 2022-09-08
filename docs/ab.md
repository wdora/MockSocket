# AB

## 2022年9月7日

PS D:\Code\GitHub> ab -n 5000 -c 10 http://localhost:8080/
This is ApacheBench, Version 2.0.40-dev <$Revision: 1.146 $> apache-2.0
Copyright 1996 Adam Twiss, Zeus Technology Ltd, http://www.zeustech.net/
Copyright 1997-2005 The Apache Software Foundation, http://www.apache.org/

Benchmarking localhost (be patient)
Completed 500 requests
Completed 1000 requests
Completed 1500 requests
Completed 2000 requests
Completed 2500 requests
Completed 3000 requests
Completed 3500 requests
Completed 4000 requests
Completed 4500 requests
Finished 5000 requests


Server Software:        Microsoft-IIS/10.0
Server Hostname:        localhost
Server Port:            8080

Document Path:          /
Document Length:        696 bytes

Concurrency Level:      10
Time taken for tests:   4.298082 seconds
Complete requests:      5000
Failed requests:        1
   (Connect: 0, Length: 1, Exceptions: 0)
Write errors:           0
Total transferred:      4814037 bytes
HTML transferred:       3479304 bytes
Requests per second:    1163.31 [#/sec] (mean)
Time per request:       8.596 [ms] (mean)
Time per request:       0.860 [ms] (mean, across all concurrent requests)
Transfer rate:          1093.74 [Kbytes/sec] received

Connection Times (ms)
              min  mean[+/-sd] median   max
Connect:        0    0   0.5      0       2
Processing:     1    7   3.2      7      20
Waiting:        0    5   2.9      5      20
Total:          1    7   3.4      7      20

Percentage of the requests served within a certain time (ms)
  50%      7
  66%      9
  75%     11
  80%     11
  90%     12
  95%     13
  98%     14
  99%     14
 100%     20 (longest request)