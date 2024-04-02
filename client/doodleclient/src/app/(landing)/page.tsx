import { Button } from "@/components/ui/button";
import Image from "next/image";
import React from "react";
import Link from "next/link";
import { Card } from "@/components/ui/card";

const LandingPage = () => {
  return (
    <div>
      <section className=" flex flex-col-reverse sm:flex-row px-11 py-28">
        <div className="md:w-[60%] max-w-xl flex flex-col justify-center space-y-5">
          <div className="bg-white pt-10 md:pt-0"></div>
          <h1 className="sm:text-left text-6xl font-bold tracking-tight text-center">
            CrispyDoodle
          </h1>
          <p className="sm:text-left text-gray-900 text-center text-lg">
            TaskFlow is not just a task scheduler; it&apos;s your personalized
            productivity navigator. Say goodbye to chaotic to-do lists and hello
            to a streamlined day ahead.
          </p>
          <div className="">
            <Button
              asChild
              className="md:px-28 w-full rounded-l-[20px] rounded-r-[20px] md:w-[45%] py-6 bg-[#22304e] hover:bg-[#22304e]/90"
            >
              <Link className="text-lg font-bold" href="/register">
                Start for free
              </Link>
            </Button>
          </div>
        </div>
        <div>
          <Image
            src={"/undraw_add_tasks_re_s5yj.svg"}
            alt="Illustration"
            width={800}
            height={500}
            className="mt-10"
          />
        </div>
      </section>
    </div>
  );
};

export default LandingPage;
