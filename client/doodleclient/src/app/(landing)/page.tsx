import { Button } from "@/components/ui/button";
import Image from "next/image";
import React from "react";
import Link from "next/link";
import { Card } from "@/components/ui/card";

const LandingPage = () => {
  return (
    <div>
      <section className=" flex flex-col-reverse sm:flex-row px-11 py-28">
        <div className="md:w-[60%] max-w-xl flex flex-col justify-center space-y-5 space-x-1">
          <div className="bg-white pt-10 md:pt-0"></div>
          <h1 className="sm:text-left text-6xl font-bold tracking-tight text-center">
            Boost your productivity and creativity.
          </h1>
          <p className="sm:text-left text-gray-900 text-center text-lg">
            CrispyDoodle is equipped with features that streamline the blogging
            process, allowing you to focus on what matters most: your ideas.
          </p>
          <div className="">
            <Button
              asChild
              className="md:px-28 w-full rounded-l-[20px] rounded-r-[20px] md:w-[45%] py-6 bg-[#0e1421] hover:bg-[#22304e]/90"
            >
              <Link className="text-lg font-bold" href="/register">
                Start for free
              </Link>
            </Button>
          </div>
        </div>
        <div>
          <Image
            src={"/landing.jpg"}
            alt="Illustration"
            width={800}
            height={500}
            className="space-x-10"
            style={{ borderRadius: "15px" }}
          />
        </div>
      </section>
    </div>
  );
};

export default LandingPage;
