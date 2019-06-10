import sys
import yaml
import tracemalloc

def invoke(submission, task, subtask, test):
    # Run in isolated context
    result = {
      	'verdict': 'FL',
      	'memory': 0,
      	'time': 0,
    }
    try:
        infile = os.joint(os.getcwd(), task, 'tests', subtask, test)
        outfile = os.joint(os.getcwd(), task, 'solutions', 'output.txt')
        sys.stdin = open(infile)
        sys.stdout = open(outfile, mode='w')
        start = datetime.now()
        tracemalloc.start()
        _, before = get_traced_memory()
        exec(source, {}, {})
        _, after = get_traced_memory()
        tracemalloc.stop()
        stop = datetime.now()
        result['time'] = (stop - start).total_seconds() * 1000
        result['memory'] = (after - before) / 1024 / 1024
    except:
        pass
    return result

      
if __name__ == '__main__':
    if len(sys.argv) < 2:
        print('Submission file is missed')
        sys.exit(0)

    # Read submission file
    try:
        f = open(sys.argv[1])
        submission = yaml.load(f)
        f.close()
    except:
        sys.exit(0)

    # for test in tests
    # invoke()

    f = open(sys.argv[1], mode='w')
    submission.dump(f)
