import subprocess


def goto(filename):
    subprocess.run(["python.exe", filename], check=True, capture_output=True, text=True)